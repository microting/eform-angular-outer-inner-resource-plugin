import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  OperationDataResult,
  OperationResult,
} from 'src/app/common/models';
import { OuterInnerResourceBaseSettingsModel } from '../models';
import { ApiBaseService } from 'src/app/common/services';

export let OuterInnerResourceSettingsMethods = {
  MachineAreaSettings: 'api/outer-inner-resource-pn/settings',
};

@Injectable({
  providedIn: 'root',
})
export class OuterInnerResourcePnSettingsService {
  constructor(private apiBaseService: ApiBaseService) {}

  getAllSettings(): Observable<
    OperationDataResult<OuterInnerResourceBaseSettingsModel>
  > {
    return this.apiBaseService.get(
      OuterInnerResourceSettingsMethods.MachineAreaSettings
    );
  }
  updateSettings(
    model: OuterInnerResourceBaseSettingsModel
  ): Observable<OperationResult> {
    return this.apiBaseService.post(
      OuterInnerResourceSettingsMethods.MachineAreaSettings,
      model
    );
  }
}
