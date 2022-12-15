import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
import {MDBBootstrapModule} from 'angular-bootstrap-md';
import {NgSelectModule} from '@ng-select/ng-select';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
  OuterInnerResourcePnReportsService,
  OuterInnerResourcePnSettingsService,
} from './services';
import {OuterInnerResourcePnLayoutComponent} from './layouts';
import {SharedPnModule} from '../shared/shared-pn.module';
import {
  OuterInnerResourceSettingsComponent,
} from './components';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {OuterInnerResourcePnRouting} from './outer-inner-resource-pn.routing';
import {OwlDateTimeModule} from '@danielmoncada/angular-datetime-picker';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    OuterInnerResourcePnRouting,
    TranslateModule,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    OwlDateTimeModule,
    EformSharedModule,
  ],
  declarations: [
    OuterInnerResourcePnLayoutComponent,
    OuterInnerResourceSettingsComponent,
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
