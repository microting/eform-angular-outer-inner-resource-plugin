# eform-angular-machinearea-plugin
Build status
[![Build Status](https://travis-ci.org/microting/eform-angular-frontend.svg?branch=master)](https://travis-ci.org/microting/eform-angular-frontend)

## NOTICE! this is not a standalone project and needs to be used with eForm Angular Frontend.

## Translating eform-angular-machinearea-plugin

| Language | C# part | Angular part |
| ------------- |:-----:|:-----:|
| English | 100% | 100%|
| Danish | 100% | 100% |
| German | 0% | 0% |

# Plugin installation tutorial
Extract zip content to a root application directory.
On front-end part plugins will be included into `eform-client/src/app/plugins/modules` To include module into front-end application add routing block to plugins.routing.module.ts


```
{
    ...
},
{
    path: 'machine-area-pn',
    canActivate: [AuthGuard],
    loadChildren: './modules/machine-area-pn/machine-area-pn.module#MachineAreaPnModule'
}
```

On the back-end part no need to do anything if you’re unpacking plugin binaries to `eFormApi/eFormAPI/Plugins`. 
If building plugin from source code – you’re need to open solution of plugin and build it in **Visual Studio 2017**.
If you’re need to implement any changes – go to `eFormAPI/Plugins/MachineArea.Pn`. Open solution, make changes and build it.
