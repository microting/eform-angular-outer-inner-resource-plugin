#!/bin/bash
sed '/\/\/ INSERT ROUTES HERE/i {' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i path: "outer-inner-resource-pn",' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i canActivate: [AuthGuard],' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i loadChildren: "./modules/outer-inner-resource-pn/outer-inner-resource-pn.module#OuterInnerResourcePnModule"' src/app/plugins/plugins.routing.ts -i
sed '/\/\/ INSERT ROUTES HERE/i },' src/app/plugins/plugins.routing.ts -i	

