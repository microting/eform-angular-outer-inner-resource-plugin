import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {ReportPnGenerateModel, ReportNamesModel} from '../../../../models';
import {format} from 'date-fns';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {OuterInnerResourcePnReportsService} from '../../../../services';
import {OuterInnerResourcePnReportTypeEnum} from '../../../../enums';
import {ExcelIcon} from 'src/app/common/const';
import {DomSanitizer} from '@angular/platform-browser';
import {MatIconRegistry} from '@angular/material/icon';

@Component({
    selector: 'app-machine-area-pn-report-generator-form',
    templateUrl: './report-generator-form.component.html',
    styleUrls: ['./report-generator-form.component.scss'],
    standalone: false
})
export class ReportGeneratorFormComponent implements OnInit {
  @Output()
  generateReport: EventEmitter<ReportPnGenerateModel> = new EventEmitter();
  @Output()
  saveReport: EventEmitter<ReportPnGenerateModel> = new EventEmitter();
  generateForm: FormGroup;
  reportNames: ReportNamesModel = new ReportNamesModel();

  get reportType() {
    return OuterInnerResourcePnReportTypeEnum;
  }

  // get relationshipTypes() { return OuterInnerResourcePnReportRelationshipEnum; }

  constructor(
    private reportService: OuterInnerResourcePnReportsService,
    iconRegistry: MatIconRegistry,
    sanitizer: DomSanitizer,
  ) {
    iconRegistry.addSvgIconLiteral('file-excel', sanitizer.bypassSecurityTrustHtml(ExcelIcon));
  }

  ngOnInit() {
    this.generateForm = new FormGroup({
      dateRange: new FormGroup({
        dateFrom: new FormControl(null, Validators.required),
        dateTo: new FormControl(null, Validators.required),
      }),
      type: new FormControl(null, Validators.required),
      relationship: new FormControl(null, Validators.required),
    });
    this.getReportNames();
  }

  onSubmit() {
    const model = this.extractData(this.generateForm.value);
    this.generateReport.emit(model);
  }

  onSave() {
    const model = this.extractData(this.generateForm.value);
    this.saveReport.emit(model);
  }

  getReportNames() {
    // this.getReportNames();
    this.reportService.getReportNames().subscribe((data) => {
      if (data && data.success) {
        this.reportNames = data.model;
      }
    });
    // return OuterInnerResourcePnReportRelationshipEnum;
  }

  private extractData(formValue: any): ReportPnGenerateModel {
    return new ReportPnGenerateModel({
      type: formValue.type,
      relationship: formValue.relationship,
      dateFrom: format(formValue.dateRange.dateFrom, 'yyyy-MM-dd'),
      dateTo: format(formValue.dateRange.dateTo, 'yyyy-MM-dd'),
    });
  }
}
