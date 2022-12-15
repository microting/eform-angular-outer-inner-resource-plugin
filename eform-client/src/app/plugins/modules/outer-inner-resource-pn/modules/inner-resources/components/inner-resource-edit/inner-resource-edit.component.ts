import {
  Component,
  EventEmitter,
  Inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  InnerResourcePnModel,
  InnerResourcePnUpdateModel,
  OuterResourcesPnModel,
} from '../../../../models';
import { OuterInnerResourcePnInnerResourceService } from '../../../../services';
import { Subscription } from 'rxjs';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService} from '@ngx-translate/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machine-edit',
  templateUrl: './inner-resource-edit.component.html',
  styleUrls: ['./inner-resource-edit.component.scss'],
})
export class InnerResourceEditComponent implements OnInit, OnDestroy {
  mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel();
  onMachineUpdated: EventEmitter<void> = new EventEmitter<void>();
  selectedMachineModel: InnerResourcePnModel = new InnerResourcePnModel();

  updateMachineSub$: Subscription;
  getSingleMachineSub$: Subscription;

  tableHeaders: MtxGridColumn[] = [
    {header: this.translateService.stream('Id'), field: 'id',},
    {header: this.translateService.stream('Name'), field: 'name',},
    {header: this.translateService.stream('External ID'), field: 'externalId',},
    {header: this.translateService.stream('Relationship'), field: 'relationship',},
  ];

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    private translateService: TranslateService,
    public dialogRef: MatDialogRef<InnerResourceEditComponent>,
    @Inject(MAT_DIALOG_DATA) model:
      { mappingAreas: OuterResourcesPnModel, machineModel: InnerResourcePnModel }
  ) {
    this.mappingAreas = model.mappingAreas;
    this.getSelectedMachine(model.machineModel.id);
  }

  ngOnInit() {}

  ngOnDestroy() {}

  getSelectedMachine(id: number) {
    this.getSingleMachineSub$ = this.machineAreaPnMachinesService
      .getSingleMachine(id)
      .subscribe((data) => {
        if (data && data.success) {
          this.selectedMachineModel = data.model;
        }
      });
  }

  updateMachine() {
    this.updateMachineSub$ = this.machineAreaPnMachinesService
      .updateMachine(new InnerResourcePnUpdateModel(this.selectedMachineModel))
      .subscribe((data) => {
        if (data && data.success) {
          this.onMachineUpdated.emit();
          this.selectedMachineModel = new InnerResourcePnModel();
          this.hide(true);
        }
      });
  }

  addToEditMapping(checked: boolean, areaId: number) {
    if (checked) {
      this.selectedMachineModel.relatedOuterResourcesIds.push(areaId);
    } else {
      this.selectedMachineModel.relatedOuterResourcesIds = this.selectedMachineModel.relatedOuterResourcesIds.filter(
        (x) => x !== areaId
      );
    }
  }

  isChecked(areaId: number) {
    if (
      this.selectedMachineModel.relatedOuterResourcesIds &&
      this.selectedMachineModel.relatedOuterResourcesIds.length > 0
    ) {
      return (
        this.selectedMachineModel.relatedOuterResourcesIds.findIndex(
          (x) => x === areaId
        ) !== -1
      );
    }
    return false;
  }

  hide(result = false) {
    this.dialogRef.close(result);
  }
}
