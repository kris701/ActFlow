// https://stackoverflow.com/questions/78822735/authorization-header-with-jwt-in-angular-to-all-requests
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

export const httpInterceptor: HttpInterceptorFn = (req, next) => {
    const messageService = inject(MessageService);

    return next(req).pipe(
        catchError((error) => {
            if (error.error && error.error.details && error.error.message)
            {
                if (error.error.details && error.error.details.length < 256) messageService.add({ severity: 'error', summary: error.error.message, detail: error.error.details, life: 10000 });
                else messageService.add({ severity: 'error', summary: 'Error', detail: error.error.message, life: 10000 });
            }
            else if (error.statusText){
                messageService.add({ severity: 'error', summary: 'Error', detail: error.statusText, life: 10000 });
            }
            return throwError(() => error);
        })
    );
};
