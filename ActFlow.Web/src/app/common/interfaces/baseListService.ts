import { HttpClient } from "@angular/common/http";
import { Directive } from "@angular/core";
import { firstValueFrom } from "rxjs";

@Directive()
export class BaseListService<T,TList> {
    public items : TList[] = [];

    public getAllEndpoint: string = '';
    public getEndpoint: string = '';

    isLoading : boolean = false;

    constructor(
        public http: HttpClient
    ) {
    }

    public async Load(){
        if (!this.isLoading){
            this.isLoading = true;
            this.items = await firstValueFrom(this.http.get<TList[]>(this.getAllEndpoint));
            this.isLoading = false;
        }
    }

    public async Get(id : string) : Promise<T> {
        var item = await firstValueFrom(this.http.get<T>(this.getEndpoint + "?id=" + id));
        return item;
    }
}
