import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {AreaPnModel, AreaPnUpdateModel} from 'src/app/plugins/modules/machine-area-pn/models/area';
import {MachinesPnModel} from 'src/app/plugins/modules/machine-area-pn/models/machine';
import {MachineAreaPnAreasService} from 'src/app/plugins/modules/machine-area-pn/services';

@Component({
  selector: 'app-machine-area-pn-area-edit',
  templateUrl: './area-edit.component.html',
  styleUrls: ['./area-edit.component.scss']
})
export class AreaEditComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingMachines: MachinesPnModel = new MachinesPnModel();
  @Output() onNewAreaDeleted: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedAreaModel: AreaPnModel = new AreaPnModel();
  constructor(private machineAreaPnAreasService: MachineAreaPnAreasService) { }

  ngOnInit() {
  }

  show(areaModel: AreaPnModel) {
    // this.getSelectedArea(areaModel.id);
    this.frame.show();
  }

  getSelectedArea(id: number) {
    this.machineAreaPnAreasService.getSingleArea(id).subscribe((data) => {
      if (data && data.success) {
        this.selectedAreaModel = data.model;
      }
    });
  }

  updateArea() {
    this.machineAreaPnAreasService.updateArea(new AreaPnUpdateModel()).subscribe((data) => {
      if (data && data.success) {
        this.onNewAreaDeleted.emit();
        this.selectedAreaModel = new AreaPnModel();
      }
    });
  }
}
