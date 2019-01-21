import { Component, OnInit } from '@angular/core';
import {ReportPnGenerateModel} from 'src/app/plugins/modules/machine-area-pn/models/report/report-pn-generate.model';
import {MachineAreaPnReportsService} from 'src/app/plugins/modules/machine-area-pn/services';

@Component({
  selector: 'app-machine-area-pn-report-generator',
  templateUrl: './report-generator-container.component.html',
  styleUrls: ['./report-generator-container.component.scss']
})
export class ReportGeneratorContainerComponent implements OnInit {
  spinnerStatus = false;
  constructor(private reportService: MachineAreaPnReportsService) {}

  ngOnInit() {
  }

  onGenerateReport(model: ReportPnGenerateModel) {
    this.spinnerStatus = true;
    this.reportService.generateReport(model).subscribe((data) => {
      if (data && data.success) {

      } this.spinnerStatus = false;
    });
  }

}
