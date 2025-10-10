interface ValidationError {
  type?: string;
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
  traceId?: string;
  message?: string;
}

export function parseValidationError(error: ValidationError): string {
  if (error.message) {
    return error.message;
  }

  if (error.errors && typeof error.errors === "object") {
    const allErrors: string[] = [];
    
    for (const field in error.errors) {
      const fieldErrors = error.errors[field];
      if (Array.isArray(fieldErrors) && fieldErrors.length > 0) {
        allErrors.push(...fieldErrors);
      }
    }

    if (allErrors.length > 0) {
      return allErrors[0];
    }
  }

  if (error.title) {
    return error.title;
  }

  return "An error occurred";
}

export function parseAllValidationErrors(error: ValidationError): string[] {
  const allErrors: string[] = [];

  if (error.message) {
    allErrors.push(error.message);
  }

  if (error.errors && typeof error.errors === "object") {
    for (const field in error.errors) {
      const fieldErrors = error.errors[field];
      if (Array.isArray(fieldErrors)) {
        allErrors.push(...fieldErrors);
      }
    }
  }

  if (allErrors.length === 0 && error.title) {
    allErrors.push(error.title);
  }

  if (allErrors.length === 0) {
    allErrors.push("An error occurred");
  }

  return allErrors;
}
