import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {
  MachineAreaPnAreasService,
  MachineAreaPnMachinesService
} from 'src/app/plugins/modules/machine-area-pn/services';
import {AreaPnCreateModel, AreaPnModel, AreasPnModel, MachinePnCreateModel, MachinesPnModel} from '../../../models';

@Component({
  selector: 'app-machine-area-pn-machine-create',
  templateUrl: './machine-create.component.html',
  styleUrls: ['./machine-create.component.scss']
})
export class MachineCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingAreas: AreasPnModel = new AreasPnModel();
  @Output() onMachineCreated: EventEmitter<void> = new EventEmitter<void>();
  checked = false;
  spinnerStatus = false;
  newMachineModel: MachinePnCreateModel = new MachinePnCreateModel();

  constructor(private machineAreaPnMachinesService: MachineAreaPnMachinesService) { }

  ngOnInit() {

  }

  show() {
    this.frame.show();
  }

  createMachine() {
    this.spinnerStatus = true;
    this.machineAreaPnMachinesService.createMachine(this.newMachineModel).subscribe((data) => {
      if (data && data.success) {
        this.onMachineCreated.emit();
        this.newMachineModel = new MachinePnCreateModel();
        this.frame.hide();
      }
      this.spinnerStatus = false;
    });
  }

  addToArray(e: any, areaId: number) {
    debugger;
    if (e.target.checked) {
      this.newMachineModel.relatedAreasIds.push(areaId);
    } else {
      this.newMachineModel.relatedAreasIds = this.newMachineModel.relatedAreasIds.filter(x => x !== areaId);
    }
  }
}
