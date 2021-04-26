import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  OperationDataResult,
  OperationResult,
} from 'src/app/common/models/operation.models';
import {
  InnerResourcePnCreateModel,
  InnerResourcePnModel,
  InnerResourcePnUpdateModel,
  InnerResourcesPnModel,
  InnerResourcesPnRequestModel,
} from '../models';
import { ApiBaseService } from 'src/app/common/services';

export let OuterInnerResourcePnInnerResourceMethods = {
  InnerResources: 'api/outer-inner-resource-pn/inner-resources',
};

@Injectable()
export class OuterInnerResourcePnInnerResourceService {
  constructor(private apiBaseService: ApiBaseService) {}

  getAllMachines(
    model: InnerResourcesPnRequestModel
  ): Observable<OperationDataResult<InnerResourcesPnModel>> {
    return this.apiBaseService.get(
      OuterInnerResourcePnInnerResourceMethods.InnerResources,
      model
    );
  }

  getSingleMachine(
    machineId: number
  ): Observable<OperationDataResult<InnerResourcePnModel>> {
    return this.apiBaseService.get(
      OuterInnerResourcePnInnerResourceMethods.InnerResources + '/' + machineId
    );
  }

  updateMachine(
    model: InnerResourcePnUpdateModel
  ): Observable<OperationResult> {
    return this.apiBaseService.put(
      OuterInnerResourcePnInnerResourceMethods.InnerResources,
      model
    );
  }

  createMachine(
    model: InnerResourcePnCreateModel
  ): Observable<OperationResult> {
    return this.apiBaseService.post(
      OuterInnerResourcePnInnerResourceMethods.InnerResources,
      model
    );
  }

  deleteMachine(machineId: number): Observable<OperationResult> {
    return this.apiBaseService.delete(
      OuterInnerResourcePnInnerResourceMethods.InnerResources + '/' + machineId
    );
  }
}
