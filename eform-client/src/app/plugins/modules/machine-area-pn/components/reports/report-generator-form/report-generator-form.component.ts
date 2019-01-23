import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {DateTimeAdapter} from 'ng-pick-datetime';
import {LocaleService} from 'src/app/common/services/auth';
import {MachineAreaPnReportTypeConst} from 'src/app/plugins/modules/machine-area-pn/enums';
import {ReportPnGenerateModel} from '../../../models';

@Component({
  selector: 'app-machine-area-pn-report-generator-form',
  templateUrl: './report-generator-form.component.html',
  styleUrls: ['./report-generator-form.component.scss']
})
export class ReportGeneratorFormComponent implements OnInit {
  @Output() generateReport: EventEmitter<ReportPnGenerateModel> = new EventEmitter();
  @Output() saveReport: EventEmitter<ReportPnGenerateModel> = new EventEmitter();
  
  get reportType() { return MachineAreaPnReportTypeConst; }
  constructor(dateTimeAdapter: DateTimeAdapter<any>,
              private localeService: LocaleService) {
    dateTimeAdapter.setLocale(this.localeService.getCurrentUserLocale());
  }

  ngOnInit() {
  }

  onSubmit(model: ReportPnGenerateModel) {
    this.generateReport.emit(model);
  }

  onSave(model: ReportPnGenerateModel) {
    this.saveReport.emit(model);
  }

}
