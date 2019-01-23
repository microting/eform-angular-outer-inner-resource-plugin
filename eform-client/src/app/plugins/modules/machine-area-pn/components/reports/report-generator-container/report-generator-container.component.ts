import { Component, OnInit } from '@angular/core';
import {ReportPnGenerateModel} from '../../../models';
import {MachineAreaPnReportsService} from '../../../services';

@Component({
  selector: 'app-machine-area-pn-report-generator',
  templateUrl: './report-generator-container.component.html',
  styleUrls: ['./report-generator-container.component.scss']
})
export class ReportGeneratorContainerComponent implements OnInit {
  reportData = {};
  spinnerStatus = false;
  constructor(private reportService: MachineAreaPnReportsService) {}

  ngOnInit() {
  }

  onGenerateReport(model: ReportPnGenerateModel) {
    this.spinnerStatus = true;
    this.reportService.generateReport(model).subscribe((data) => {
      if (data && data.success) {
        this.reportData = data.model;
      } this.spinnerStatus = false;
    });
  }

  onSaveReport(model: ReportPnGenerateModel) {
    this.spinnerStatus = true;
    this.reportService.getGeneratedReport(model).subscribe((data) => {
      if (data && data.success) {

      } this.spinnerStatus = false;
    });
  }

}
