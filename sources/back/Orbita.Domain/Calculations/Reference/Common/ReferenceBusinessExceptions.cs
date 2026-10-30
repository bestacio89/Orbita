namespace Orbita.Domain.Calculations.Reference.Common;

/// <summary>Raised when a business rule rejects a calculation input.</summary>
public class BusinessRuleException(string message) : Exception(message);

/// <summary>The supplied workbook does not contain a usable TSP.</summary>
public sealed class InvalidTspException(string message) : BusinessRuleException(message);
