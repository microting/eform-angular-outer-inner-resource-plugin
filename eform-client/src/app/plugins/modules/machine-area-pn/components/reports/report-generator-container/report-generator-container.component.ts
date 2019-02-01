import {Component, OnInit} from '@angular/core';
import {ReportPnFullModel, ReportPnGenerateModel} from '../../../models';
import {MachineAreaPnReportsService} from '../../../services';
import {saveAs} from 'file-saver';
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-machine-area-pn-report-generator',
  templateUrl: './report-generator-container.component.html',
  styleUrls: ['./report-generator-container.component.scss']
})
export class ReportGeneratorContainerComponent implements OnInit {
  reportModel: ReportPnFullModel = new ReportPnFullModel();
  spinnerStatus = false;

  constructor(private reportService: MachineAreaPnReportsService, private toastrService: ToastrService) {
  }

  ngOnInit() {
  }

  onGenerateReport(model: ReportPnGenerateModel) {
    this.spinnerStatus = true;
    this.reportService.generateReport(model).subscribe((data) => {
      if (data && data.success) {
        this.reportModel = data.model;
      }
      this.spinnerStatus = false;
    });
  }

  onSaveReport(model: ReportPnGenerateModel) {
    this.spinnerStatus = true;
    this.reportService.getGeneratedReport(model).subscribe(((data) => {
      saveAs(data, model.dateFrom + '_' + model.dateTo + '_report.xlsx');
      this.spinnerStatus = false;
    }), error => {
      this.toastrService.error();
      this.spinnerStatus = false;
    });
  }
}
