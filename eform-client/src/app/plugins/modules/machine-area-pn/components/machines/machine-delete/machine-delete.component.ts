import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  selector: 'app-machine-area-pn-machine-delete',
  templateUrl: './machine-delete.component.html',
  styleUrls: ['./machine-delete.component.scss']
})
export class MachineDeleteComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onNewMachineDeleted: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedMachineModel: {
    id: 1,
    name: ''
  };
  constructor() { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  deleteMachine() {

  }

}
