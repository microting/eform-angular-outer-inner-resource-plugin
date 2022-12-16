import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OperationDataResult } from 'src/app/common/models';
import {
  ReportPnFullModel,
  ReportNamesModel,
  ReportPnGenerateModel,
} from '../models';
import { ApiBaseService } from 'src/app/common/services';

export let OuterInnerResourcePnReportsMethods = {
  Reports: 'api/outer-inner-resource-pn/reports',
};

@Injectable({
  providedIn: 'root',
})
export class OuterInnerResourcePnReportsService {
  constructor(private apiBaseService: ApiBaseService) {}

  generateReport(
    model: ReportPnGenerateModel
  ): Observable<OperationDataResult<ReportPnFullModel>> {
    return this.apiBaseService.get(
      OuterInnerResourcePnReportsMethods.Reports,
      model
    );
  }

  getReportNames(): Observable<OperationDataResult<ReportNamesModel>> {
    return this.apiBaseService.get(
      OuterInnerResourcePnReportsMethods.Reports + '/reportnames'
    );
  }

  getGeneratedReport(model: ReportPnGenerateModel): Observable<any> {
    return this.apiBaseService.getBlobData(
      OuterInnerResourcePnReportsMethods.Reports + '/excel',
      model
    );
  }
}
