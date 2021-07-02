using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using EFHooks;

namespace Infonet.Core.Entity {
	public abstract class EnhancedDbContext : HookedDbContext {
		protected EnhancedDbContext(string nameOrConnectionString) : base(nameOrConnectionString) {
			InitializeHooks();
		}

		// ReSharper disable once UnusedMember.Global
		protected EnhancedDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) {
			InitializeHooks();
		}

		// ReSharper disable once UnusedMember.Global
		protected EnhancedDbContext() {
			InitializeHooks();
		}

		private void InitializeHooks() {
			RegisterHook(new DeleteIfNulledHook());
			RegisterHook(new RemoveIfTrueHook());
			RegisterHook(new RevisableHook());
		}

		public override int SaveChanges() {
			try {
				var entities = new Dictionary<object, EntityState>();
				foreach (var each in ChangeTracker.Entries())
					entities[each.Entity] = each.State;

				Database.LogLine(Environment.NewLine + "SAVING CHANGES");
				foreach (var each in entities)
					Database.LogLine("\t{0}\t{1}", each.Value, each.Key);
				Database.LogLine();

				int result = base.SaveChanges();

				/* notify entities that SaveChanges completed successfully */
				foreach (var each in entities) {
					var receiver = each.Key as INotifyContextSavedChanges;
					receiver?.OnContextSavedChanges(each.Value);
				}

				return result;
			} catch (DbEntityValidationException e) {
				foreach (var eve in e.EntityValidationErrors) {
					Database.LogLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach (var ve in eve.ValidationErrors)
						Database.LogLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
				}
				throw;
			}
		}

		private class RevisableHook : PreActionHook<IRevisable> {
			public override bool RequiresValidation {
				get { return true; }
			}

			public override EntityState HookStates {
				get { return EntityState.Added | EntityState.Modified; }
			}

			public override void Hook(IRevisable entity, HookEntityMetadata metadata) {
				entity.RevisionStamp = DateTime.Now; //KMS DO this should lookup db time for first call and then adjust db time using current server time for subsequent calls
			}
		}

		private class DeleteIfNulledHook : PreActionHook<object> {
			public override bool RequiresValidation {
				get { return false; }
			}

			public override EntityState HookStates {
				get { return EntityState.Modified; }
			}

			public override void Hook(object entity, HookEntityMetadata metadata) {
				foreach (DeleteIfNulledAttribute each in entity.GetType().GetCustomAttributes(typeof(DeleteIfNulledAttribute), true))
					if (each.AppliesTo(entity)) {
						metadata.CurrentContext.Set(entity.GetType()).Remove(entity);
						metadata.CurrentContext.Database.LogLine("\t[DeleteIfNulled] DELETED " + entity);
					}
			}
		}

		private class RemoveIfTrueHook : PreActionHook<object> {
			public override bool RequiresValidation {
				get { return false; }
			}

			public override EntityState HookStates {
				get { return EntityState.Added | EntityState.Modified; }
			}

			public override void Hook(object entity, HookEntityMetadata metadata) {
				foreach (RemoveIfTrueAttribute each in entity.GetType().GetCustomAttributes(typeof(RemoveIfTrueAttribute), true))
					if (each.AppliesTo(entity)) {
						metadata.CurrentContext.Set(entity.GetType()).Remove(entity);
						metadata.CurrentContext.Database.LogLine($"\t[RemoveIfTrue] {(metadata.State == EntityState.Modified ? "DELETED" : "DETACHED")} {entity}");
					}
			}
		}
	}
}