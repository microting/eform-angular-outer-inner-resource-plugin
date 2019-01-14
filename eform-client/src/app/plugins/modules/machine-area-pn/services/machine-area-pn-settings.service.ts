import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {OperationDataResult, OperationResult} from '../../../../common/models';
import {MachineAreaSettingsModel} from '../models';
import {BaseService} from '../../../../common/services/base.service';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';

export let MachineAreaSettingsMethods = {
  MachineAreaSettings: 'api/machine-area-pn/settings'

};
@Injectable()
export class MachineAreaPnSettingsService extends BaseService {

  constructor(private _http: HttpClient, router: Router, toastrService: ToastrService) {
    super(_http, router, toastrService);
  }

  getAllSettings(): Observable<OperationDataResult<MachineAreaSettingsModel>> {
    return this.get(MachineAreaSettingsMethods.MachineAreaSettings);
  }
  updateSettings(model: MachineAreaSettingsModel): Observable<OperationResult> {
    return this.post(MachineAreaSettingsMethods.MachineAreaSettings, model);
  }
}
