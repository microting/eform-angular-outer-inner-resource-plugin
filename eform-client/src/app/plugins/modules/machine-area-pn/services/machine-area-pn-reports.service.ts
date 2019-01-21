import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {OperationDataResult, OperationResult} from 'src/app/common/models/operation.models';
import {BaseService} from 'src/app/common/services/base.service';

export let MachineAreaPnReportsMethods = {
  Reports: 'api/machine-area-pn/reports',
};

@Injectable()
export class MachineAreaPnReportsService extends BaseService {
  constructor(private _http: HttpClient, router: Router, toastrService: ToastrService) {
    super(_http, router, toastrService);
  }

  generateReport(model: any): Observable<OperationDataResult<any>> {
    return this.post(MachineAreaPnReportsMethods.Reports, model);
  }

}
