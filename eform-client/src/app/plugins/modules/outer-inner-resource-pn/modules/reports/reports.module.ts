import {NgModule} from '@angular/core';
import {MatDialogModule} from '@angular/material/dialog';
import {
  ReportGeneratorContainerComponent,
  ReportGeneratorFormComponent,
  ReportPreviewTableComponent,
} from './components';
import {ReportsRouting} from './reports.routing';

@NgModule({
  imports: [
    MatDialogModule,
    ReportGeneratorContainerComponent,
    ReportGeneratorFormComponent,
    ReportPreviewTableComponent,
    ReportsRouting,
  ],
  providers: [],
})
export class ReportsModule {
}
