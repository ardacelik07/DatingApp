
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    register= false;
    users: any;
  constructor() { }

  ngOnInit(): void {
    
  }

  registerToggle(){
    this.register = !this.register;
  }

  
  cancelRegisterMode(event: boolean){
    this.register = event;
  }
}
