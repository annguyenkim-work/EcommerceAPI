using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Common.Behaviors;

// TRequest: Bất kỳ Command/Query nào gửi vào MediatR
// TResponse: Kết quả trả về tương ứng
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra xem Request này có Validator nào được định nghĩa không
        if (!_validators.Any())
        {
            return await next(); // Không có validator thì cho đi tiếp vào Handler
        }

        // 2. Tạo context để validate
        var context = new ValidationContext<TRequest>(request);

        // 3. Chạy tất cả các Validator (Bất đồng bộ)
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // 4. Gom tất cả các lỗi lại thành một danh sách
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // 5. Nếu có lỗi, ném ra ValidationException
        // Ngoại lệ này sẽ được ExceptionHandlingMiddleware ở lớp API "chộp" lấy và trả về RFC 7807
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        // 6. Nếu dữ liệu sạch, cho phép Request đi tiếp vào Handler nghiệp vụ
        return await next();
    }
}