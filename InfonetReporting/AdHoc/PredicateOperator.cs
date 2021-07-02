namespace Infonet.Reporting.AdHoc {
	//KMS DO rename?  maybe LogicOperation or LogicalOperator or ...
	//KMS DO is this even needed?

	//KMS ORs inside of ANDs need ()s
	//KMS ANDs inside of ORs don't...though ()'s make it more clear
	public enum PredicateOperator {
		Comparison,
		And,
		Or
	}
}