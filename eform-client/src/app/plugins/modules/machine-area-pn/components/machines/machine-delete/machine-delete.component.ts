import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {MachinePnModel} from '../../../models';
import {MachineAreaPnMachinesService} from '../../../services';

@Component({
  selector: 'app-machine-area-pn-machine-delete',
  templateUrl: './machine-delete.component.html',
  styleUrls: ['./machine-delete.component.scss']
})
export class MachineDeleteComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onMachineDeleted: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedMachineModel: MachinePnModel = new MachinePnModel();
  constructor(private machineAreaPnMachinesService: MachineAreaPnMachinesService) { }

  ngOnInit() {
  }

  show(machineModel: MachinePnModel) {
    this.selectedMachineModel = machineModel;
    this.frame.show();
  }

  deleteMachine() {
    this.machineAreaPnMachinesService.deleteMachine(this.selectedMachineModel.id).subscribe((data) => {
      if (data && data.success) {
        this.onMachineDeleted.emit();
        this.selectedMachineModel = new MachinePnModel();
      }
    });
  }

}
