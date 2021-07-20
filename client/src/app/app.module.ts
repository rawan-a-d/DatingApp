import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { FormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';

// Decorator for Angular Module
@NgModule({
  // Components
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent
  ],
  // Angular Modules
  imports: [
    BrowserModule,
    AppRoutingModule,
    // http requests
    HttpClientModule,
    TooltipModule.forRoot(),
    BrowserAnimationsModule,
    FormsModule,
    // drop down 
    // forRoot: initializes services, components with the root module
    // https://valor-software.com/ngx-bootstrap/#/dropdowns
    BsDropdownModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
