import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {TranslateModule} from '@ngx-translate/core';
import {
  ReportGeneratorContainerComponent,
  ReportGeneratorFormComponent,
  ReportPreviewTableComponent,
} from './components';
import {ReportsRouting} from './reports.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MtxGridModule} from '@ng-matero/extensions/grid';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MtxSelectModule} from '@ng-matero/extensions/select';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepickerModule} from '@angular/material/datepicker';

@NgModule({
  declarations: [
    ReportGeneratorContainerComponent,
    ReportGeneratorFormComponent,
    ReportPreviewTableComponent,
  ],
  imports: [
    TranslateModule,
    RouterModule,
    ReportsRouting,
    EformSharedModule,
    FormsModule,
    ReactiveFormsModule,
    MtxGridModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MtxSelectModule,
    MatButtonModule,
    MatDatepickerModule,
  ],
  providers: [],
})
export class ReportsModule {
}
