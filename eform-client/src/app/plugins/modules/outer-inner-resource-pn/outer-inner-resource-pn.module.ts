import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
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
import {OuterInnerResourcePnRouting} from './outer-inner-resource-pn.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MtxSelectModule} from '@ng-matero/extensions/select';
import {MatCardModule} from '@angular/material/card';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    OuterInnerResourcePnRouting,
    TranslateModule,
    FormsModule,
    EformSharedModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MtxSelectModule,
    MatCardModule,
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
