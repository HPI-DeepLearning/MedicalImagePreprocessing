// wrpMedicalImageIO.h

#pragma once
#include "MedicalImage.h"

using namespace System;

namespace wrpMedicalImageIO {

	public ref class MedicalIO
	{
	public:
		static MedicalImage^ ReadImage(System::String^ filename);
	private :
		static void MarshalString(System::String ^ filename, std::string& nativefilename);
	};
}
