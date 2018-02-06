#include <string>
#include <iostream>
#include "..\modMedicalImageIO\MedImg.h"


#pragma once

public ref class MedicalImage
{
public:
	property int ndim{ int get() { return (this->nativeimg->ndim); }; } /*!< dimensions of grid array             */
	property int nx{ int get() { return (this->nativeimg->nx); }; }
	property int ny{ int get() { return (this->nativeimg->ny); }; }
	property int nz{ int get() { return (this->nativeimg->nz); }; }
	property int nt{ int get() { return (this->nativeimg->nt); }; }
	property int nu{ int get() { return (this->nativeimg->nu); }; }
	property int nv{ int get() { return (this->nativeimg->nv); }; }
	property int nw{ int get() { return (this->nativeimg->nw); }; }
	property size_t nvox{ size_t get() { return (this->nativeimg->nvox); }; }
	//property array<int>^ dim{ array<int>^ get() { return (this->nativeimg->dim); }; }



	property int nbyper{ int get() { return (this->nativeimg->nbyper); }; }
	property int datatype{ int get() { return (this->nativeimg->datatype); }; }

	property float dx{ float get() { return (this->nativeimg->dx); }; }
	property float dy{ float get() { return (this->nativeimg->dy); }; }
	property float dz{ float get() { return (this->nativeimg->dz); }; }
	property float dt{ float get() { return (this->nativeimg->dt); }; }
	property float du{ float get() { return (this->nativeimg->du); }; }
	property float dv{ float get() { return (this->nativeimg->dv); }; }
	property float dw{ float get() { return (this->nativeimg->dw); }; }
	
	property float scl_slope{ float get() { return (this->nativeimg->scl_slope); }; }
	property float scl_inter{ float get() { return (this->nativeimg->scl_inter); }; }
	property float cal_min{ float get() { return (this->nativeimg->cal_min); }; }
	property float cal_max{ float get() { return (this->nativeimg->cal_max); }; }

	property int qform_code{ int get() { return (this->nativeimg->qform_code); }; }
	property int sform_code{ int get() { return (this->nativeimg->sform_code); }; }

	property int freq_dim{ int get() { return (this->nativeimg->freq_dim); }; }
	property int phase_dim{ int get() { return (this->nativeimg->phase_dim); }; }
	property int slice_dim{ int get() { return (this->nativeimg->slice_dim); }; }
	property int slice_code{ int get() { return (this->nativeimg->slice_code); }; }
	property int slice_start{ int get() { return (this->nativeimg->slice_start); }; }
	property int slice_end{ int get() { return (this->nativeimg->slice_end); }; }

	property float slice_duration{ float get() { return (this->nativeimg->slice_duration); }; }
	property float quatern_b{ float get() { return (this->nativeimg->quatern_b); }; }
	property float quatern_c{ float get() { return (this->nativeimg->quatern_c); }; }
	property float quatern_d{ float get() { return (this->nativeimg->quatern_d); }; }
	property float qoffset_x{ float get() { return (this->nativeimg->qoffset_x); }; }
	property float qoffset_y{ float get() { return (this->nativeimg->qoffset_y); }; }
	property float qoffset_z{ float get() { return (this->nativeimg->qoffset_z); }; }
	property float qfac{ float get() { return (this->nativeimg->qfac); }; }
	property float toffset{ float get() { return (this->nativeimg->toffset); }; }

	property int xyz_units{ int get() { return (this->nativeimg->xyz_units); }; }
	property int time_units{ int get() { return (this->nativeimg->time_units); }; }
	property int nifti_type{ int get() { return (this->nativeimg->nifti_type); }; }
	property int intent_code{ int get() { return (this->nativeimg->intent_code); }; }

	property float intent_p1{ float get() { return (this->nativeimg->intent_p1); }; }
	property float intent_p2{ float get() { return (this->nativeimg->intent_p2); }; }
	property float intent_p3{ float get() { return (this->nativeimg->intent_p3); }; }

	property int iname_offset{ int get() { return (this->nativeimg->iname_offset); }; }
	property int swapsize{ int get() { return (this->nativeimg->swapsize); }; }
	property int byteorder{ int get() { return (this->nativeimg->byteorder); }; }

	property int num_ext{ int get() { return (this->nativeimg->num_ext); }; }

	//array<int>^ dim;                  /*!< dim[0]=ndim, dim[1]=nx, etc.         */
	//
	//array<float>^ pixdim;             /*!< pixdim[1]=dx, etc. */

	//array<char>^ intent_name;       /*!< optional description of intent data */

	//array<char>^ descrip;           /*!< optional text to describe dataset   */
	//array<char>^ aux_file;           /*!< auxiliary filename                  */

	////char *fname;                 /*!< header filename (.hdr or .nii)         */
	////char *iname;                 /*!< image filename  (.img or .nii)         */
	property void* data{ void* get() { return (this->nativeimg->data); }; }

	////nifti1_extension * ext_list; /*!< array of extension structs (with data) */
	////analyze_75_orient_code analyze75_orient; /*!< for old analyze files, orient */

	MedicalImage(int width,int height, int numberofslides, int numbyteper);
	MedicalImage();
	MedicalImage(std::string nativefilename);

	void NiftiReconstruction(System::Drawing::Bitmap^ bmp, int x, int y, int z);
	void Setsclides(System::Drawing::Bitmap^ bmp, int index);
	void SavetoNifti(System::String ^ filename);

	System::Drawing::Bitmap^ TransversalPlaneReconstruction(int indx);
	System::Drawing::Bitmap^ SagittalPlaneReconstruction(int indx);
	System::Drawing::Bitmap^ CoronalPlaneReconstruction(int indx);


private:
	MedImg* nativeimg;
};

