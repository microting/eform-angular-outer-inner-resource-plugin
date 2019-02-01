import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {OperationDataResult, OperationResult} from 'src/app/common/models/operation.models';
import {BaseService} from 'src/app/common/services/base.service';
import {ReportPnFullModel, ReportPnGenerateModel} from '../models';

export let MachineAreaPnReportsMethods = {
  Reports: 'api/machine-area-pn/reports',
};

@Injectable()
export class MachineAreaPnReportsService extends BaseService {
  constructor(private _http: HttpClient, router: Router, toastrService: ToastrService) {
    super(_http, router, toastrService);
  }

  generateReport(model: ReportPnGenerateModel): Observable<OperationDataResult<ReportPnFullModel>> {
    return this.get(MachineAreaPnReportsMethods.Reports, model);
  }

  getGeneratedReport(model: ReportPnGenerateModel): Observable<OperationDataResult<any>> {
    return this.getBlobData(MachineAreaPnReportsMethods.Reports, model);
  }

}
