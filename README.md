# AC9-Samples
This repository provides source code for sample plugins for AbleCommerce shopping cart software

## How to build the sample project

Git clone or download and unzip the source code. This solution was created with **Visual Studio 2015** so you will be needing the same version of higher to build the sample codes. In order to compile and build install artifacts you will have to follow the following steps.

* Open the *AC9-Samples.sln* solution file in Visual Studio, **Right Click** on **solution name** in **Solution Explorer** and then choose **Restore NuGet Packages** command. This should restore most of the dependencies needed by these sample projects.

* Create a folder called **Lib** inside your solution folder. Then copy **CommerceBuilder.dll** and **CommerceBuilder.Web.dll** files from your **AbleCommerce Shopping Cart Application**, you will find these DLLs in your website's *Bin* folder. This should resolve all dependencies required by sample projects.

* Build a sample plugin project in Visual Studio for example try to build **ExamplePaymentPlugin** project. Once build successfully completes it will create a folder named **Build** within project folder with installer zip file. In this case it will be **ExamplePaymentPlugin.zip**

* Copy **ExamplePaymentPlugin.zip** to your **Website/Plugins** folder, login into administration panel and go to **Plugins** area. Your newly added **ExamplePaymentPlugin** will be listed in there ready to install.
