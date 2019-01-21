import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {DateTimeAdapter} from 'ng-pick-datetime';
import {LocaleService} from 'src/app/common/services/auth';
import {ReportPnGenerateModel} from 'src/app/plugins/modules/machine-area-pn/models/report/report-pn-generate.model';

@Component({
  selector: 'app-machine-area-pn-report-generator-form',
  templateUrl: './report-generator-form.component.html',
  styleUrls: ['./report-generator-form.component.scss']
})
export class ReportGeneratorFormComponent implements OnInit {
  @Output() generateReport: EventEmitter<ReportPnGenerateModel> = new EventEmitter();
  constructor(dateTimeAdapter: DateTimeAdapter<any>,
              private localeService: LocaleService) {
    dateTimeAdapter.setLocale(this.localeService.getCurrentUserLocale());
  }

  ngOnInit() {
  }

  onSubmit(model: ReportPnGenerateModel) {
    debugger;
    this.generateReport.emit(model);
  }

}
