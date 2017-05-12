// TransferMode.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <Windows.h>
#include <ntddscsi.h> // for ATA_PASS_THROUGH_EX
#include <iostream>

int main(int argc, char* argv[])
{
	HANDLE handle = CreateFileA(
		"\\\\.\\PhysicalDrive0",
		GENERIC_READ | GENERIC_WRITE, //IOCTL_ATA_PASS_THROUGH requires read-write
		FILE_SHARE_READ,
		0,            //no security attributes
		OPEN_EXISTING,
		0,              //flags and attributes
		0             //no template file
		);

	if (handle == INVALID_HANDLE_VALUE)
	{
		std::cout << "Invalid handle" << std::endl;
		return 0;
	}

	// IDENTIFY command requires a 512 byte buffer for data:
	const unsigned int IDENTIFY_buffer_size = 512;
	const BYTE IDENTIFY_command_ID = 0xEC;
	unsigned char Buffer[IDENTIFY_buffer_size + sizeof(ATA_PASS_THROUGH_EX)] = { 0 };
	ATA_PASS_THROUGH_EX & PTE = *(ATA_PASS_THROUGH_EX *)Buffer;
	PTE.Length = sizeof(PTE);
	PTE.TimeOutValue = 10;
	PTE.DataTransferLength = 512;
	PTE.DataBufferOffset = sizeof(ATA_PASS_THROUGH_EX);

	// Set up the IDE registers as specified in ATA spec.
	IDEREGS * ir = (IDEREGS *)PTE.CurrentTaskFile;
	ir->bCommandReg = IDENTIFY_command_ID;
	ir->bSectorCountReg = 1;

	// IDENTIFY is neither 48-bit nor DMA, it reads from the device:
	PTE.AtaFlags = ATA_FLAGS_DATA_IN | ATA_FLAGS_DRDY_REQUIRED;

	DWORD BR = 0;
	BOOL b = ::DeviceIoControl(handle, IOCTL_ATA_PASS_THROUGH, &PTE, sizeof(Buffer), &PTE, sizeof(Buffer), &BR, 0);
	if (b == 0)
	{
		std::cout << "Invalid call DeviceIoControl" << std::endl;;
		CloseHandle(handle);
		return 0;
	}

	WORD * data = (WORD *)(Buffer + sizeof(ATA_PASS_THROUGH_EX));
	int interface_id = data[222];
	if ((data[80] >> 8) == 0)
	{
		if (data[93] == 0)
			std::cout << "Disk is old SATA" << std::endl;
		else
			std::cout << "Disk is PATA" << std::endl;
	}
	else if ((interface_id >> 12) == 0)
	{
		std::cout << "Disk is PATA" << std::endl;
	}
	else if ((interface_id >> 12) == 1)
	{
		std::cout << "Disk is SATA" << std::endl;
		const char *sata_ver[] = { "ATA8-AST", "SATA 1.0a", "SATA II: Extensions", "SATA Rev 2.5", "SATA Rev 2.6", "SATA 3.0", "SATA 3.1",
			"Reserved", "Reserved", "Reserved", "Reserved", "Reserved" };
		interface_id &= 0x7FF;
		int bit_idx = 0;
		while (interface_id > 0)
		{
			if ((interface_id & 0x01) != 0)
				std::cout << sata_ver[bit_idx] << std::endl;
			interface_id >>= 1;
			bit_idx++;
		}
	}
	else
	{
		std::cout << "Disk is interface is unknown" << std::endl;
	}
	CloseHandle(handle);
	return 0;
}