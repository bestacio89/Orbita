namespace Orbita.Domain.Common;

/// <summary>Raised when a domain operation violates a business invariant.</summary>
public class BusinessRuleException(string message) : Exception(message);

/// <summary>Raised when an imported TSP workbook cannot be interpreted as a supported document.</summary>
public sealed class InvalidTspException(string message) : BusinessRuleException(message);
