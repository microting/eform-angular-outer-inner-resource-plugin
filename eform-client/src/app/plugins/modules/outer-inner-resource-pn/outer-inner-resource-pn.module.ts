import {NgModule} from '@angular/core';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
  OuterInnerResourcePnReportsService,
  OuterInnerResourcePnSettingsService,
} from './services';
import {OuterInnerResourcePnLayoutComponent} from './layouts';
import {
  OuterInnerResourceSettingsComponent,
} from './components';
import {OuterInnerResourcePnRouting} from './outer-inner-resource-pn.routing';
import {StoreModule} from '@ngrx/store';
import {outerResourceReducer, innerResourcesReducer} from './state';

@NgModule({
  imports: [
    OuterInnerResourcePnRouting,
    OuterInnerResourcePnLayoutComponent,
    OuterInnerResourceSettingsComponent,
    StoreModule.forFeature('outerInnerResourcePn', {
      outerResourcesState: outerResourceReducer,
      innerResourcesState: innerResourcesReducer,
    }),
  ],
  providers: [
    OuterInnerResourcePnOuterResourceService,
    OuterInnerResourcePnInnerResourceService,
    OuterInnerResourcePnSettingsService,
    OuterInnerResourcePnReportsService,
  ],
})
export class OuterInnerResourcePnModule {
}
