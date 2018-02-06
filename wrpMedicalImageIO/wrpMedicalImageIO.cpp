// This is the main DLL file.

#include "stdafx.h"
#include "MedicalImage.h"
#include "wrpMedicalImageIO.h"

namespace wrpMedicalImageIO {
	/*static*/
	MedicalImage^ MedicalIO::ReadImage(System::String^ filename)
	{
		std::string nativefilename;

		MarshalString(filename, nativefilename);

		auto img = gcnew MedicalImage(nativefilename);

		return img;
	}

	/*static*/
	void MedicalIO::MarshalString(System::String ^ filename, std::string& nativefilename)
	{
		using namespace Runtime::InteropServices;
		const char* chars =
			(const char*)(Marshal::StringToHGlobalAnsi(filename)).ToPointer();
		nativefilename = chars;
		Marshal::FreeHGlobal(IntPtr((void*)chars));

	}

}