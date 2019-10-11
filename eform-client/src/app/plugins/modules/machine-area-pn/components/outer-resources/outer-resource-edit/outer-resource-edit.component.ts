import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';

import {OuterResourcePnModel, OuterResourcePnUpdateModel} from '../../../models/area';
import {InnerResourcesPnModel} from '../../../models/machine';
import {OuterInnerResourcePnOuterResourceService} from '../../../services';

@Component({
  selector: 'app-machine-area-pn-area-edit',
  templateUrl: './outer-resource-edit.component.html',
  styleUrls: ['./outer-resource-edit.component.scss']
})
export class OuterResourceEditComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();
  @Output() onAreaUpdated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedAreaModel: OuterResourcePnModel = new OuterResourcePnModel();
  constructor(private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService) { }

  ngOnInit() {
  }

  show(areaModel: OuterResourcePnModel) {
    this.getSelectedArea(areaModel.id);
    this.frame.show();
  }

  getSelectedArea(id: number) {
    this.spinnerStatus = true;
    this.machineAreaPnAreasService.getSingleArea(id).subscribe((data) => {
      if (data && data.success) {
        this.selectedAreaModel = data.model;
      } this.spinnerStatus = false;
    });
  }

  updateArea() {
    this.spinnerStatus = true;
    this.machineAreaPnAreasService.updateArea(new OuterResourcePnUpdateModel(this.selectedAreaModel)).subscribe((data) => {
      if (data && data.success) {
        this.onAreaUpdated.emit();
        this.selectedAreaModel = new OuterResourcePnModel();
        this.frame.hide();
      } this.spinnerStatus = false;
    });
  }

  addToEditMapping(e: any, machineId: number) {
    if (e.target.checked) {
      this.selectedAreaModel.relatedMachinesIds.push(machineId);
    } else {
      this.selectedAreaModel.relatedMachinesIds = this.selectedAreaModel.relatedMachinesIds.filter(x => x !== machineId);
    }
  }

  isChecked(machineId: number) {
    if (this.selectedAreaModel.relatedMachinesIds && this.selectedAreaModel.relatedMachinesIds.length > 0) {
      return this.selectedAreaModel.relatedMachinesIds.findIndex(x => x === machineId) !== -1;
    } return false;
  }
}
