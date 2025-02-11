import {Component, Input, OnChanges, OnInit, SimpleChanges, TemplateRef} from '@angular/core';
import {OuterInnerResourcePnReportRelationshipEnum} from '../../../../enums';
import {ReportEntityModel, ReportPnFullModel} from '../../../../models';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService} from '@ngx-translate/core';

@Component({
    selector: 'app-machine-area-pn-report-preview-table',
    templateUrl: './report-preview-table.component.html',
    styleUrls: ['./report-preview-table.component.scss'],
    standalone: false
})
export class ReportPreviewTableComponent implements OnInit, OnChanges {
  @Input() reportData: ReportPnFullModel = new ReportPnFullModel();
  @Input() toolbarTemplate!: TemplateRef<any>;
  tableHeaders: MtxGridColumn[] = [];
  data: ReportEntityModel[] = [];

  constructor(
    private translateService: TranslateService,
  ) {
  }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.reportData && changes.reportData.currentValue) {
      this.tableHeaders = [
        {header: this.translateService.stream('Entity'), field: 'entityName', summary: () => `<strong>${this.translateService.instant('Sum')}</strong>`},
      ];
      if (this.reportData.relationship === OuterInnerResourcePnReportRelationshipEnum.EmployeeInnerResource
        || this.reportData.relationship === OuterInnerResourcePnReportRelationshipEnum.EmployeeOuterResource) {
        this.tableHeaders = [
          ...this.tableHeaders,
          {header: this.translateService.stream('Relationship'), field: 'relatedEntityName'},
        ];
      }
      this.tableHeaders = [
        ...this.tableHeaders,
        {
          header: this.translateService.stream('Sum'),
          field: 'totalTime',
          summary: data => `<strong>${data.reduce((previousValue, currentValue) => previousValue + currentValue, 0)}</strong>`,
          type: 'number',
          typeParameter: {
            digitsInfo: '1.0-2',
          },
        },
        ...this.reportData.reportHeaders.map((x, i): MtxGridColumn => (
          {
            header: x.headerValue,
            field: `timePerTimeUnit.${i}`,
            summary: data => `<strong>${data.reduce((previousValue, currentValue) => previousValue + currentValue, 0)}</strong>`,
            type: 'number',
            typeParameter: {
              digitsInfo: '1.0-2',
            },
          })),
      ];
      this.data = [];
      this.reportData.subReports.forEach(subReport => {
        subReport.entities.forEach(entityModel => {
          this.data.push({...entityModel});
        });
      });
    }
  }
}
