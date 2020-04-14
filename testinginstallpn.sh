#!/bin/bash
perl -pi -e '$_.="  },\n" if /INSERT ROUTES HERE/' src/app/plugins/plugins.routing.ts
perl -pi -e '$_.="      .then(m => m.OuterInnerResourcePnModule)\n" if /INSERT ROUTES HERE/' src/app/plugins/plugins.routing.ts
perl -pi -e '$_.="    loadChildren: () => import('\''./modules/outer-inner-resource-pn/outer-inner-resource-pn.module'\'')\n" if /INSERT ROUTES HERE/' src/app/plugins/plugins.routing.ts
perl -pi -e '$_.="    path: '\''outer-inner-resource-pn'\'',\n" if /INSERT ROUTES HERE/' src/app/plugins/plugins.routing.ts
perl -pi -e '$_.="  {\n" if /INSERT ROUTES HERE/' src/app/plugins/plugins.routing.ts
