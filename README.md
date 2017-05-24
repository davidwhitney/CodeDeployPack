# CodeDeployPack

Generates an Amazon CodeDeploy.zip package as part of your standard MSBuild.
Adds scripts to your CodeDeploy manifest file based on standard conventions.

- [Usage](#usage)
- [Supported Conventions](#conventions)

---

<a name="usage"></a>
## Usage

1. Add a package reference to "CodeDeployPack" from NuGet
2. Call MSBuild with the following parameters


```bash
    msbuild /t:Clean;Build /p:RunCodeDeployPack=true
```

Result:

A valid CodeDeploy.zip file will be generated in:

    c:\YourSln\YourCsProj\obj\packed\CodeDeploy.zip

---

<a name="conventions"></a>
## Supported Conventions

By default, all script files should be included in a `Content Directory` called `.deploy` in your application.

The files included in this directory should be **all marked as content**.

Upon build, any files matching the following name patterns, will automagically be bundled and invoked by the `AWS CodeDeploy` process.

* before-install.*
* after-install.*
* application-start.*
* application-stop.*
* validate-service.*