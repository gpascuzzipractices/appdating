import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  //@Input() usersFromHomeComponent:any; non viene iù usato
  @Output() cancelRegister = new EventEmitter();
  model:any={} //inizializzo valore tipo any oggeto vuoto

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe({
      next: () =>{
        //console.log(response);
        this.cancel();
      },
      error:error =>{console.log(error);window.alert(error.error.title);}
    })
          
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
