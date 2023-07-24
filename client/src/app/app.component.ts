
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Prova App';
  

  constructor(private accountService: AccountService) { }
  //Possiamo aggiungere codice iniziale
  ngOnInit(): void {
    setTimeout(this.removeSpinner, 2000);
    this.setCurrentUser();
  }

  

  setCurrentUser(){
    const userString= localStorage.getItem('user');
    if (!userString) return;
    const user : User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }

  // app.ts
  removeSpinner() {
  const spinnerContainer = document.querySelector('.loading-container');
  spinnerContainer?.classList.remove('show-spinner');
  }

// Wait for 2 seconds (2000 milliseconds) and then remove the spinner
  





}
