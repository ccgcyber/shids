#include "stdafx.h"
#include <urlmon.h>
#include <conio.h>

#include <IO.h>

#include "LimitSingleInstance.H"

#include "DetectDotNet.h"

class MyCallback : public IBindStatusCallback 
{
public:
	MyCallback() {}

	~MyCallback() { }

	// This one is called by URLDownloadToFile
	STDMETHOD(OnProgress)(/* [in] */ ULONG ulProgress, /* [in] */ ULONG ulProgressMax, /* [in] */ ULONG ulStatusCode, /* [in] */ LPCWSTR wszStatusText)
	{
		//cout << "Downloaded " << ulProgress << " of " << ulProgressMax << " byte(s), " << " Status Code = " << ulStatusCode << endl;
		cout << "=";
		return S_OK;
	}

	// The rest don't do anything...
	STDMETHOD(OnStartBinding)(/* [in] */ DWORD dwReserved, /* [in] */ IBinding __RPC_FAR *pib)
	{ return E_NOTIMPL; }

	STDMETHOD(GetPriority)(/* [out] */ LONG __RPC_FAR *pnPriority)
	{ return E_NOTIMPL; }

	STDMETHOD(OnLowResource)(/* [in] */ DWORD reserved)
	{ return E_NOTIMPL; }

	STDMETHOD(OnStopBinding)(/* [in] */ HRESULT hresult, /* [unique][in] */ LPCWSTR szError)
	{ return E_NOTIMPL; }

	STDMETHOD(GetBindInfo)(/* [out] */ DWORD __RPC_FAR *grfBINDF, /* [unique][out][in] */ BINDINFO __RPC_FAR *pbindinfo)
	{ return E_NOTIMPL; }

	STDMETHOD(OnDataAvailable)(/* [in] */ DWORD grfBSCF, /* [in] */ DWORD dwSize, /* [in] */ FORMATETC __RPC_FAR *pformatetc, /* [in] */ STGMEDIUM __RPC_FAR *pstgmed)
	{ return E_NOTIMPL; }

	STDMETHOD(OnObjectAvailable)(/* [in] */ REFIID riid, /* [iid_is][in] */ IUnknown __RPC_FAR *punk)
	{ return E_NOTIMPL; }

	// IUnknown stuff
	STDMETHOD_(ULONG,AddRef)()
	{ return 0; }

	STDMETHOD_(ULONG,Release)()
	{ return 0; }

	STDMETHOD(QueryInterface)(/* [in] */ REFIID riid, /* [iid_is][out] */ void __RPC_FAR *__RPC_FAR *ppvObject)
	{ return E_NOTIMPL; }
};

void downloadNet(){

	cout<<"\n=>Installing Windows Installer KB893803-v2-x86"<<endl;
	system("WindowsInstaller.exe /quiet /norestart");
	DeleteFile("WindowsInstaller.exe");

	cout<<"\n=>Installing Dot Net Framework 2.0"<<endl;
	system("dotnet20.exe /q");
	DeleteFile("dotnet20.exe");

}

CLimitSingleInstance g_SingleInstanceObj(TEXT("Global\\{9DA0BEED-7248-450a-B27C-C0409BDC377D}"));

int _tmain(int, _TCHAR**)
{
	//Only one instance
	if (g_SingleInstanceObj.IsAnotherInstanceRunning()){
		cout<<"One instance only"<<endl;
       return 0;
	}

		CDetectDotNet detect;

		bool dowonloadingNeeded=true;
		vector<string> CLRVersions;
		if(detect.IsDotNetPresent()){
			detect.EnumerateCLRVersions(CLRVersions);
			for(vector<string>::iterator it = CLRVersions.begin(); 
				it < CLRVersions.end(); it++)
			{
				try
				{
					string x=(*it).c_str();
					string ss=x.substr (0,1);
					if(ss=="2"){
						dowonloadingNeeded=false;
					}else if(ss=="3"){
						dowonloadingNeeded=false;
					}
					else if(ss=="4"){
						dowonloadingNeeded=false;
					}
				}
				catch (int e){}
				if(!dowonloadingNeeded){
					break;
				}	
			}
		}
		cout<<"S|HIDS Installer"<<endl;
		cout<<"----------------------"<<endl;
		if(dowonloadingNeeded==true){
			downloadNet();
		}		

	return 0;
}
