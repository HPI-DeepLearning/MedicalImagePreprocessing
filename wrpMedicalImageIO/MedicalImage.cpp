#include "stdafx.h"
#include "MedicalImage.h"
#include "..\modMedicalImageIO\MedImgIO.h"
#include "itkImageFileWriter.h"
#include "itkNiftiImageIO.h"

using namespace std;
using namespace System;

MedicalImage::MedicalImage()
{
	this->nativeimg = new MedImg();
}

MedicalImage::MedicalImage(std::string nativefilename)
{
	this->nativeimg = MedImgIO::imread(nativefilename);

}


MedicalImage::MedicalImage(int width, int height, int numberofslides, int numbyteper)
{
	this->nativeimg = new MedImg(width, height, numberofslides, numbyteper);
}

void MedicalImage::Setsclides(System::Drawing::Bitmap^ bmp, int index)
{
	auto npixel = bmp->Width * bmp->Height;
	auto data2D = gcnew array<unsigned char>(3 * npixel);
	auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
	System::Runtime::InteropServices::Marshal::Copy(bmpdata->Scan0, data2D, 0, 3 * npixel);
	bmp->UnlockBits(bmpdata);
	size_t offset3D = index* bmp->Width * bmp->Height;
	size_t offset2D = 0;
	auto data3D = static_cast<unsigned char*>(this->nativeimg->data);	

	for (int it = 0; it < npixel; it++)
	{
         auto val = data2D[offset2D];
		 data3D[offset3D] = val;
		 offset3D = offset3D + 1;
		 offset2D = offset2D + 3;////for RGB
	}


}

void MedicalImage::SavetoNifti(System::String ^ filename)
{
	using namespace Runtime::InteropServices;
	const char* chars =
		(const char*)(Marshal::StringToHGlobalAnsi(filename)).ToPointer();
	std::string nativefilename = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));




	const unsigned int Dimension = 3;
	typedef itk::Image< short, Dimension > ImageType;
	ImageType::Pointer image = ImageType::New();

	const ImageType::SizeType size = { { this->nx, this->ny, this->nz } }; //SIZE{x,y,z}
	const ImageType::IndexType start = { { 0, 0, 0 } }; //index on {x,y,z}

	ImageType::RegionType region;
	region.SetSize(size);
	region.SetIndex(start);

	//bmp ro bekhun beriz tu itk 


	/*image = arr;*/
	image->SetRegions(region);
	image->Allocate(true);

	ImageType::SpacingType spacing;
	spacing[0] = 1; // 0.33;
	spacing[1] = 1; //0.33;
	spacing[2] = 1; //1.20;

	image->SetSpacing(spacing);
	const ImageType::SpacingType& sp = image->GetSpacing();
	std::cout << "Spacing = ";
	std::cout << sp[0] << ", " << sp[1] << ", " << sp[2] << std::endl;

	ImageType::PointType newOrigin;
	newOrigin.Fill(0.0);
	image->SetOrigin(newOrigin);

	const ImageType::PointType & origin = image->GetOrigin();
	std::cout << "Origin = ";
	std::cout << origin[0] << ", " << origin[1] << ", " << origin[2] << std::endl;


	ImageType::DirectionType direction;
	direction.SetIdentity();
	image->SetDirection(direction);

	const ImageType::DirectionType& direct = image->GetDirection();
	std::cout << "Direction = " << std::endl;
	std::cout << direct << std::endl;

	typedef itk::Point< double, ImageType::ImageDimension > PointType;
	PointType point;
	point[0] = 1.45;    // x coordinate
	point[1] = 7.21;    // y coordinate
	point[2] = 9.28;    // z coordinate

	auto data3D = static_cast<unsigned char*>(this->nativeimg->data);
	auto offset3D = 0;

		
	itk::ImageRegionIterator<ImageType> imageIterator(image, region);

	while (!imageIterator.IsAtEnd())
	{
		// Get the value of the current pixel
		//unsigned char val = imageIterator.Get();
		//std::cout << (int)val << std::endl;

		// Set the current pixel to white
		auto val = static_cast<unsigned char>(data3D[offset3D]);
		imageIterator.Set(val);
		++offset3D;
		++imageIterator;
	}



	std::string outputFilename = nativefilename;
	//outputFilename = savefilename;
	//outputFilename = "D:\\code-nifti\\Brats17_2013_1_1\\Brats17_2013_1_1.nii.gz";
	typedef  itk::ImageFileWriter< ImageType  > WriterType;
	WriterType::Pointer writer = WriterType::New();
	writer->SetFileName(outputFilename);
	writer->SetInput(image);
	writer->Update();


}

void MedicalImage::NiftiReconstruction(System::Drawing::Bitmap^ bmp, int x, int y, int z)
{
	const unsigned int Dimension = 3;
	typedef itk::Image< unsigned short, Dimension > ImageType;
	ImageType::Pointer image = ImageType::New();

	const ImageType::SizeType size = { { x, y, z } }; //SIZE{x,y,z}
	const ImageType::IndexType start = { { 0, 0, 0 } }; //index on {x,y,z}

	ImageType::RegionType region;
	region.SetSize(size);
	region.SetIndex(start);

    //bmp ro bekhun beriz tu itk 

	auto npixel = bmp->Width * bmp->Height;
	auto data2D = gcnew array<unsigned char>(3*npixel);
	auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
	System::Runtime::InteropServices::Marshal::Copy(bmpdata->Scan0, data2D,0, 3*npixel);
	bmp->UnlockBits(bmpdata);
	

	/*image = arr;*/
	image->SetRegions(region);
	image->Allocate(true);

	ImageType::SpacingType spacing;
	spacing[0] = 0.33;
	spacing[1] = 0.33;
	spacing[2] = 1.20;

	image->SetSpacing(spacing);
	const ImageType::SpacingType& sp = image->GetSpacing();
	std::cout << "Spacing = ";
	std::cout << sp[0] << ", " << sp[1] << ", " << sp[2] << std::endl;

	ImageType::PointType newOrigin;
	newOrigin.Fill(0.0);
	image->SetOrigin(newOrigin);

	const ImageType::PointType & origin = image->GetOrigin();
	std::cout << "Origin = ";
	std::cout << origin[0] << ", " << origin[1] << ", " << origin[2] << std::endl;


	ImageType::DirectionType direction;
	direction.SetIdentity();
	image->SetDirection(direction);

	const ImageType::DirectionType& direct = image->GetDirection();
	std::cout << "Direction = " << std::endl;
	std::cout << direct << std::endl;

	typedef itk::Point< double, ImageType::ImageDimension > PointType;
	PointType point;
	point[0] = 1.45;    // x coordinate
	point[1] = 7.21;    // y coordinate
	point[2] = 9.28;    // z coordinate

	//ImageType::IndexType pixelIndex;
	//const bool isInside =
	// image->TransformPhysicalPointToIndex(point, pixelIndex);
	//if (isInside)
	//{
	// ImageType::PixelType pixelValue = image->GetPixel(pixelIndex);
	// pixelValue += 5;
	// image->SetPixel(pixelIndex, pixelValue);
	//}

	//itk::NiftiImageIO::Pointer nifti_io = itk::NiftiImageIO::New();
	//nifti_io->SetPixelType(pixel_type(imageIO));

	itk::ImageRegionIterator<ImageType> imageIterator(image, region);

	while (!imageIterator.IsAtEnd())
	{
		// Get the value of the current pixel
		//unsigned char val = imageIterator.Get();
		//std::cout << (int)val << std::endl;

		// Set the current pixel to white
		imageIterator.Set(255);

		++imageIterator;
	}

	
	std::string outputFilename;
	outputFilename = "Y:\\PhD-research\\DataSet\\braindatabase\\Brats17TrainingData\\1seg-2\\test.nii.gz";
	typedef  itk::ImageFileWriter< ImageType  > WriterType;
	WriterType::Pointer writer = WriterType::New();
	writer->SetFileName(outputFilename);
	writer->SetInput(image);
	writer->Update();


}

System::Drawing::Bitmap^ MedicalImage::TransversalPlaneReconstruction(int planeIndx)
{
	if (planeIndx > nz)
		throw new exception("indx > nz");
	auto width = this->nx;
	auto height = ny;
	float inch2mm = 25.40f;
	float xDpi = inch2mm / Convert::ToSingle(dx);
	float yDpi = inch2mm / Convert::ToSingle(dy);
	//cout << "dx dy dz = \t" << dx << "\t" << dy << "\t" << dz << endl;
	//cout << "xDpi yDpi = \t" << xDpi << "\t" << yDpi << endl;
	int pixelsnumber = width*height;
	// byte order is not checked
	// only one type of data raster is supported
	switch (nbyper)
	{
	case 1:
	{
		/*cout << "nbyper = " << nbyper << endl;
		throw new exception("nbyper > 1");
		break;*/
		auto data3D = static_cast<unsigned char*>(this->nativeimg->data);
		auto data2D = gcnew array<unsigned char>(pixelsnumber);
		size_t off = planeIndx*(width*height);
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = off + ity * width + itx;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format8bppIndexed);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}
	case 2:
	{
		auto data3D = static_cast<short*>(this->nativeimg->data);
		auto data2D = gcnew array<short>(pixelsnumber);
		size_t off = planeIndx*(width*height);
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = off + ity*width + itx;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format16bppGrayScale);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}

	case 4:
	{
		auto data3D = static_cast<float*>(this->nativeimg->data);
		auto data2D = gcnew array<float>(pixelsnumber);
		size_t off = planeIndx*(width*height);
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = off + ity*width + itx;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format32bppRgb); //Format32bppRgb
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		//if (planeIndx == 90){ bmp->Save("C:\\Users\\Minaa\\Documents\\90.png"); }
		
		return bmp;
		break;
	}

	case 8:
	{

		//if (planeIndx > nz)
		//	throw new exception("indx > ny");
		//auto width = this->nx;
		//auto height = nz;
		//float inch2mm = 25.40f;
		//float xDpi = inch2mm / Convert::ToSingle(dx);
		//float yDpi = inch2mm / Convert::ToSingle(dz);
		////cout << "dx dy dz = \t" << dx << "\t" << dy << "\t" << dz << endl;
		////cout << "xDpi yDpi = \t" << xDpi << "\t" << yDpi << endl;
		//int pixelsnumber = width*height;

		//auto data3D = static_cast<double*>(this->nativeimg->data);
		//auto data2D = gcnew array<double>(pixelsnumber);
		//size_t off = planeIndx*this->ny;
		//for (size_t ity = 0; ity < height; ity++)
		//{
		//	for (size_t itx = 0; itx < width; itx++)
		//	{
		//		size_t offset3D = ity*(this->nx*this->ny) + off + itx;;
		//		size_t offset2D = ity * width + itx;
		//		data2D[offset2D] = data3D[offset3D];
		//		//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
		//	}
		//}
		//auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format64bppArgb);
		//auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		//System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		//bmp->UnlockBits(bmpdata);
		//bmp->SetResolution(xDpi, yDpi);
		//return bmp;
		//break;



		auto data3D = static_cast<double*>(this->nativeimg->data);
		auto data2D = gcnew array<double>(pixelsnumber);
		size_t off = planeIndx*(width*height);

		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D =  off + ity * width + itx ;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format64bppArgb);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		if (planeIndx == 1){ bmp->Save("Y:\\PhD-research\\1.png"); }

		return bmp;
		break;
	}

	default:
		cout << "nbyper = " << nbyper << endl;
		throw new exception("nbyper > 1");
		break;
	}
}

System::Drawing::Bitmap^ MedicalImage::SagittalPlaneReconstruction(int planeIndx)
{
	if (planeIndx > ny)
		throw new exception("indx > ny");
	auto width = this->nx;
	auto height = nz;
	float inch2mm = 25.40f;
	float xDpi = inch2mm / Convert::ToSingle(dx);
	float yDpi = inch2mm / Convert::ToSingle(dz);
	//cout << "dx dy dz = \t" << dx << "\t" << dy << "\t" << dz << endl;
	//cout << "xDpi yDpi = \t" << xDpi << "\t" << yDpi << endl;
	int pixelsnumber = width*height;
	// byte order is not checked
	// only one type of data raster is supported
	switch (nbyper)
	{
	case 1:
	{
		auto data3D = static_cast<unsigned char*>(this->nativeimg->data);
		auto data2D = gcnew array<unsigned char>(pixelsnumber);
		size_t off = planeIndx*this->ny;
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = ity*(this->nx*this->ny) + off + itx;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format8bppIndexed);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}
	case 2:
	{
		auto data3D = static_cast<short*>(this->nativeimg->data);
		auto data2D = gcnew array<short>(pixelsnumber);
		size_t off = planeIndx*this->ny;
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = ity*(this->nx*this->ny) + off + itx ;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format16bppGrayScale);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}

	case 8:
	{
		auto data3D = static_cast<double*>(this->nativeimg->data);
		auto data2D = gcnew array<double>(pixelsnumber);
		size_t off = planeIndx*this->ny;
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = ity*(this->nx*this->ny) + off + itx;
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				////cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format64bppArgb);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}

	

	default:
		cout << "nbyper = " << nbyper << endl;
		throw new exception("nbyper > 1");
		break;
	}
}

System::Drawing::Bitmap^ MedicalImage::CoronalPlaneReconstruction(int planeIndx)
{

	if (planeIndx > nx)
		throw new exception("planeIndx > nx");
	auto width = this->ny;
	auto height = nz;
	float inch2mm = 25.40f;
	float xDpi = inch2mm / Convert::ToSingle(dy);
	float yDpi = inch2mm / Convert::ToSingle(dz);
	//cout << "dx dy dz = \t" << dx << "\t" << dy << "\t" << dz << endl;
	//cout << "xDpi yDpi = \t" << xDpi << "\t" << yDpi << endl;
	int pixelsnumber = width*height;
	// byte order is not checked
	// only one type of data raster is supported
	// only 8bit per pixel images are supported not 16bit per pixel
	switch (nbyper)
	{
	case 1:
	{
		auto data3D = static_cast<unsigned char*>(this->nativeimg->data);
		auto data2D = gcnew array<unsigned char>(pixelsnumber);
		size_t off = planeIndx;
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = planeIndx + itx*(this->nx) + ity * (this->ny*this->nx);
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format8bppIndexed);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}
	case 2:
	{
		auto data3D = static_cast<short*>(this->nativeimg->data);
		auto data2D = gcnew array<short>(pixelsnumber);
		size_t off = planeIndx;
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = planeIndx + itx*(this->nx) + ity * (this->ny*this->nx);
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format16bppGrayScale);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}


	case 8:
	{
		auto data3D = static_cast<double*>(this->nativeimg->data);
		auto data2D = gcnew array<double>(pixelsnumber);
		size_t off = planeIndx;
		for (size_t ity = 0; ity < height; ity++)
		{
			for (size_t itx = 0; itx < width; itx++)
			{
				size_t offset3D = planeIndx + itx*(this->nx) + ity * (this->ny*this->nx);
				size_t offset2D = ity * width + itx;
				data2D[offset2D] = data3D[offset3D];
				//cout << ity << "\t" << itx << "\t" << offset3D << "\t" << data3D[offset3D] << endl;
			}
		}
		auto bmp = gcnew System::Drawing::Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format64bppArgb);
		auto bmpdata = bmp->LockBits(System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height), System::Drawing::Imaging::ImageLockMode::ReadWrite, bmp->PixelFormat);
		System::Runtime::InteropServices::Marshal::Copy(data2D, 0, bmpdata->Scan0, pixelsnumber);
		bmp->UnlockBits(bmpdata);
		bmp->SetResolution(xDpi, yDpi);
		return bmp;
		break;
	}

	default:
		cout << "nbyper = " << nbyper << endl;
		throw new exception("nbyper > 1");
		break;
	}
}
