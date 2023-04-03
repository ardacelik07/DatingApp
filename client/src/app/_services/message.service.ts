import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { GetPaginationHeaders, getPaginatedResult } from './paginationHelper';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
 baseUrl = environment.apiUrl;
  constructor(private Http: HttpClient) { }

  getMessages(pageNumber:number,pageSize: number,container:string){
    let params = GetPaginationHeaders(pageNumber,pageSize);
    params = params.append('Container',container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages',params,this.Http);
  }


  getMessageThread(username: string ){
    return this.Http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }
  sendMessage(username : string , content : string){
    return this.Http.post<Message>(this.baseUrl + 'messages',{recipientUsername: username,content})
  }
  deleteMessage(id :number){
   return this.Http.delete(this.baseUrl + 'messages/' + id);
  }
}
