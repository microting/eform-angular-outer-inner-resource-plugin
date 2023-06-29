import {
  Component,
  EventEmitter,
  Inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  OuterResourcePnModel,
  OuterResourcePnUpdateModel,
  InnerResourcesPnModel,
} from '../../../../models';
import {OuterInnerResourcePnOuterResourceService} from '../../../../services';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';
import {Subscription} from 'rxjs';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService} from '@ngx-translate/core';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-area-edit',
  templateUrl: './outer-resource-edit.component.html',
  styleUrls: ['./outer-resource-edit.component.scss'],
})
export class OuterResourceEditComponent implements OnInit, OnDestroy {
  mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();
  onAreaUpdated: EventEmitter<void> = new EventEmitter<void>();
  selectedAreaModel: OuterResourcePnModel = new OuterResourcePnModel();

  updateAreaSub$: Subscription;
  getSingleAreaSub$: Subscription;

  tableHeaders: MtxGridColumn[] = [
    {header: this.translateService.stream('Id'), field: 'id',},
    {header: this.translateService.stream('Name'), field: 'name',},
    {header: this.translateService.stream('Relationship'), field: 'relationship',},
  ];

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    private translateService: TranslateService,
    public dialogRef: MatDialogRef<OuterResourceEditComponent>,
    @Inject(MAT_DIALOG_DATA) model:
      { mappingMachines: InnerResourcesPnModel, areaModel: OuterResourcePnModel }
  ) {
    this.mappingMachines = model.mappingMachines;
    this.getSelectedArea(model.areaModel.id);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  getSelectedArea(id: number) {
    this.getSingleAreaSub$ = this.machineAreaPnAreasService
      .getSingleArea(id)
      .subscribe((data) => {
        if (data && data.success) {
          this.selectedAreaModel = data.model;
        }
      });
  }

  updateArea() {
    this.updateAreaSub$ = this.machineAreaPnAreasService
      .updateArea(new OuterResourcePnUpdateModel(this.selectedAreaModel))
      .subscribe((data) => {
        if (data && data.success) {
          this.onAreaUpdated.emit();
          this.selectedAreaModel = new OuterResourcePnModel();
          this.hide(true);
        }
      });
  }

  addToEditMapping(checked: boolean, machineId: number) {
    if (checked) {
      this.selectedAreaModel.relatedInnerResourcesIds.push(machineId);
    } else {
      this.selectedAreaModel.relatedInnerResourcesIds = this.selectedAreaModel.relatedInnerResourcesIds.filter(
        (x) => x !== machineId
      );
    }
  }

  isChecked(machineId: number) {
    if (
      this.selectedAreaModel.relatedInnerResourcesIds &&
      this.selectedAreaModel.relatedInnerResourcesIds.length > 0
    ) {
      return (
        this.selectedAreaModel.relatedInnerResourcesIds.findIndex(
          (x) => x === machineId
        ) !== -1
      );
    }
    return false;
  }

  hide(result = false) {
    this.dialogRef.close(result);
  }
}
