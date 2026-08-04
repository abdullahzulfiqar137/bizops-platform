import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '../state/auth-state.service';

export const permissionGuard: CanActivateFn = (route, state) => {
  const authState = inject(AuthStateService);
  const router = inject(Router);

  const requiredRoles = route.data?.['roles'] as string[] | undefined;

  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  if (authState.hasAnyRole(requiredRoles)) {
    return true;
  }

  return router.createUrlTree(['/forbidden'], {
    queryParams: { returnUrl: state.url }
  });
};
