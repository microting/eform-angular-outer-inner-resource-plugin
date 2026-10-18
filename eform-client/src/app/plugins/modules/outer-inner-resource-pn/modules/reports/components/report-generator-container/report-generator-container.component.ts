import {EformNewSubheaderComponent} from 'src/app/common/modules/eform-shared/components/eform-new-subheader/eform-new-subheader.component';
import {ReportGeneratorFormComponent} from '../report-generator-form/report-generator-form.component';
import {ReportPreviewTableComponent} from '../report-preview-table/report-preview-table.component';
import { Component, OnInit } from '@angular/core';
import { ReportPnFullModel, ReportPnGenerateModel } from '../../../../models';
import { OuterInnerResourcePnReportsService } from '../../../../services';
import { saveAs } from 'file-saver';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-machine-area-pn-report-generator',
    templateUrl: './report-generator-container.component.html',
    imports: [EformNewSubheaderComponent, ReportGeneratorFormComponent, ReportPreviewTableComponent],
    styleUrls: ['./report-generator-container.component.scss']
})
export class ReportGeneratorContainerComponent implements OnInit {
  reportModel: ReportPnFullModel = new ReportPnFullModel();

  constructor(
    private reportService: OuterInnerResourcePnReportsService,
    private toastrService: ToastrService
  ) {}

  ngOnInit() {}

  onGenerateReport(model: ReportPnGenerateModel) {
    this.reportService.generateReport(model).subscribe((data) => {
      if (data && data.success) {
        this.reportModel = data.model;
      }
    });
  }

  onSaveReport(model: ReportPnGenerateModel) {
    this.reportService.getGeneratedReport(model).subscribe(
      (data) => {
        saveAs(data, model.dateFrom + '_' + model.dateTo + '_report.xlsx');
      },
      (error) => {
        this.toastrService.error();
      }
    );
  }
}
