import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  OperationDataResult,
  OperationResult,
} from 'src/app/common/models/operation.models';
import {
  OuterResourcePnCreateModel,
  OuterResourcePnModel,
  OuterResourcePnUpdateModel,
  OuterResourcesPnModel,
  OuterResourcesPnRequestModel,
} from '../models';
import { ApiBaseService } from 'src/app/common/services';

export let OuterInnerResourcePnOuterResourceMethods = {
  Areas: 'api/outer-inner-resource-pn/outer-resources',
};

@Injectable()
export class OuterInnerResourcePnOuterResourceService {
  constructor(private apiBaseService: ApiBaseService) {}

  getAllAreas(
    model: OuterResourcesPnRequestModel
  ): Observable<OperationDataResult<OuterResourcesPnModel>> {
    return this.apiBaseService.get(
      OuterInnerResourcePnOuterResourceMethods.Areas,
      model
    );
  }

  getSingleArea(
    areaId: number
  ): Observable<OperationDataResult<OuterResourcePnModel>> {
    return this.apiBaseService.get(
      OuterInnerResourcePnOuterResourceMethods.Areas + '/' + areaId
    );
  }

  updateArea(model: OuterResourcePnUpdateModel): Observable<OperationResult> {
    return this.apiBaseService.put(
      OuterInnerResourcePnOuterResourceMethods.Areas,
      model
    );
  }

  createArea(model: OuterResourcePnCreateModel): Observable<OperationResult> {
    return this.apiBaseService.post(
      OuterInnerResourcePnOuterResourceMethods.Areas,
      model
    );
  }

  deleteArea(machineId: number): Observable<OperationResult> {
    return this.apiBaseService.delete(
      OuterInnerResourcePnOuterResourceMethods.Areas + '/' + machineId
    );
  }
}
