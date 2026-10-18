import {NgModule} from '@angular/core';
import {
  ReportGeneratorContainerComponent,
  ReportGeneratorFormComponent,
  ReportPreviewTableComponent,
} from './components';
import {ReportsRouting} from './reports.routing';

@NgModule({
  imports: [
    ReportGeneratorContainerComponent,
    ReportGeneratorFormComponent,
    ReportPreviewTableComponent,
    ReportsRouting,
  ],
  providers: [],
})
export class ReportsModule {
}
