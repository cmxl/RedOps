using FluentValidation;

namespace RedOps.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Project name is required and cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.RedmineId)
            .GreaterThan(0)
            .When(x => x.RedmineId.HasValue)
            .WithMessage("Redmine ID must be greater than 0.");

        RuleFor(x => x.AzureDevOpsProject)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.AzureDevOpsProject))
            .WithMessage("Azure DevOps project name cannot exceed 100 characters.");

        RuleFor(x => x.SyncDirection)
            .IsInEnum()
            .WithMessage("Invalid sync direction.");

        RuleFor(x => x)
            .Must(HaveAtLeastOneMapping)
            .WithMessage("At least one external system mapping (Redmine or Azure DevOps) is required.");
    }

    private static bool HaveAtLeastOneMapping(CreateProjectCommand command)
    {
        return command.RedmineId.HasValue || !string.IsNullOrEmpty(command.AzureDevOpsProject);
    }
}