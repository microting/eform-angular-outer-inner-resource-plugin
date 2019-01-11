#!/bin/bash
cd ../eform-angular-frontend/eform-client/src/app/plugins
sed '/\/\/ INSERT ROUTES HERE/i {' plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i path: "machine-area-pn",' plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i canActivate: [AuthGuard],' plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i loadChildren: "./modules/machine-area-pn/machine-area-pn.module#MachineAreaPnModule"' plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i }' plugins.routing.ts -i

