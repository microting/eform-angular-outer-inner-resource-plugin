import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {OperationDataResult, OperationResult} from 'src/app/common/models/operation.models';
import {BaseService} from 'src/app/common/services/base.service';
import {
  InnerResourcePnCreateModel,
  InnerResourcePnModel,
  InnerResourcesPnRequestModel, InneResourcePnUpdateModel,
  InnerResourcesPnModel
} from '../models';
import {ReportNamesModel} from '../models/report/report-names.model';

export let OuterInnerResourcePnInnerResourceMethods = {
  Machines: 'api/outer-inner-resource-pn/inner-resources',
};

@Injectable()
export class OuterInnerResourcePnInnerResourceService extends BaseService {
  constructor(private _http: HttpClient, router: Router, toastrService: ToastrService) {
    super(_http, router, toastrService);
  }

  getAllMachines(model: InnerResourcesPnRequestModel): Observable<OperationDataResult<InnerResourcesPnModel>> {
    return this.get(OuterInnerResourcePnInnerResourceMethods.Machines, model);
  }

  getSingleMachine(machineId: number): Observable<OperationDataResult<InnerResourcePnModel>> {
    return this.get(OuterInnerResourcePnInnerResourceMethods.Machines + '/' + machineId);
  }

  updateMachine(model: InneResourcePnUpdateModel): Observable<OperationResult> {
    return this.put(OuterInnerResourcePnInnerResourceMethods.Machines, model);
  }

  createMachine(model: InnerResourcePnCreateModel): Observable<OperationResult> {
    return this.post(OuterInnerResourcePnInnerResourceMethods.Machines, model);
  }

  deleteMachine(machineId: number): Observable<OperationResult> {
    return this.delete(OuterInnerResourcePnInnerResourceMethods.Machines + '/' + machineId);
  }

}
